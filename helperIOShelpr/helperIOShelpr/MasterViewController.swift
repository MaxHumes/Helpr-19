//
//  MasterViewController.swift
//  helperIOShelpr
//
//  Created by Sam Horn on 4/4/20.
//  Copyright © 2020 Sam Horn. All rights reserved.
//

import UIKit
import CoreData

class MasterViewController: UITableViewController, NSFetchedResultsControllerDelegate {

    var detailViewController: DetailViewController? = nil
    var managedObjectContext: NSManagedObjectContext? = (UIApplication.shared.delegate as? AppDelegate)?.persistentContainer.viewContext
    var newThreadView: NewThreadView? = nil
    var threadView: ThreadView? = nil
    var token: Data? = nil
    var threads: [Thread]? = nil
    
    
    struct Thread: Codable {
        var thread_id = -1
        var name = ""
        var description = ""
    }

    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view.
        //navigationItem.leftBarButtonItem = editButtonItem

        let threadButton = UIBarButtonItem(barButtonSystemItem: .compose, target: self, action: #selector(insertNewObject(_:)))
        //let threadButton = UIBarButtonItem(barButtonSystemItem: .compose, target: self, action: #selector(threadSegue()))
        navigationItem.rightBarButtonItem = threadButton
        if let split = splitViewController {
            let controllers = split.viewControllers
            detailViewController = (controllers[controllers.count-1] as! UINavigationController).topViewController as? DetailViewController
            //detailViewController?.managedObjectContext = managedObjectContext
        }
    }

    override func viewWillAppear(_ animated: Bool) {
        clearsSelectionOnViewWillAppear = splitViewController!.isCollapsed
        super.viewWillAppear(animated)
        
    }
    
    
    override func viewDidAppear(_ animated: Bool) {
        
        let semaphore = DispatchSemaphore (value: 0)
        
        guard let url = URL(string: "https://helpr19api.azurewebsites.net/api/users/login") else { return }
        
        var request = URLRequest(url: url, timeoutInterval: Double.infinity)
        request.httpMethod = "GET"
        request.addValue("application/json", forHTTPHeaderField: "Content-Type")
        
        let task = URLSession.shared.dataTask(with: request) { data, response, error in
            guard let data = data else {
                print(String(describing: error))
                return
            }
            
            guard let response = (response as! HTTPURLResponse?) else {
                print(String(describing: error))
                return
            }
           // print(String(data: data, encoding: .utf8)!)
            //print("\n\n\nresponse: \(String(describing: response))")
            let decoder = JSONDecoder()
            
            do {
                self.threads = try decoder.decode([Thread].self, from: data)
            } catch {
                print(error.localizedDescription)
            }
            semaphore.signal()
        }
        
        task.resume()
        semaphore.signal()
        
        
        //if newThreadView?.wasRecentView ?? true {
          //  let context = self.fetchedResultsController.managedObjectContext
            //let newThread = Thread(context: context)
        
            //newThread.timestamp = Date()
            //newThread.thread_name = newThreadView?.threadText
            
            //let newPost = Post(context: context)
            //newPost.message = newThreadView?.messageText
            //newPost.thread = newThread
            //newThread.addToPosts(newPost)
            /*
            do {
                try context.save()
            } catch {
                // Replace this implementation with code to handle the error appropriately.
                // fatalError() causes the application to generate a crash log and terminate. You should not use this function in a shipping application, although it may be useful during development.
                let nserror = error as NSError
                fatalError("Unresolved error \(nserror), \(nserror.userInfo)")
            }
            newThreadView?.wasRecentView = false
         
 */
//let parameters: [String: String]
        
        super.viewDidAppear(animated)
        
    }
     
    
    // func addView() {
    //   insertNewObject(_:)
    // addSubview
    //}
    

    @objc func insertNewObject(_ sender: Any) {
        performSegue(withIdentifier: "newThread", sender: self)
    }
    
    /*
     @objc
     func threadSegue() {
     performSegue(withIdentifier: "newThread", sender: self)
     let context = self.fetchedResultsController.managedObjectContext
     do {
     try context.save()
     } catch {
     // Replace this implementation with code to handle the error appropriately.
     // fatalError() causes the application to generate a crash log and terminate. You should not use this function in a shipping application, although it may be useful during development.
     let nserror = error as NSError
     fatalError("Unresolved error \(nserror), \(nserror.userInfo)")
     }
     }
     */
    /*
     
     @objc
     func insertNewObject(_ sender: Any) {
     let context = self.fetchedResultsController.managedObjectContext
     //let newThread = Thread(context: context)
     
     // If appropriate, configure the new managed object.
     //newThread.timestamp = Date()
     
     // Save the context.
     do {
     try context.save()
     } catch {
     // Replace this implementation with code to handle the error appropriately.
     // fatalError() causes the application to generate a crash log and terminate. You should not use this function in a shipping application, although it may be useful during development.
     let nserror = error as NSError
     fatalError("Unresolved error \(nserror), \(nserror.userInfo)")
     }
     performSegue(withIdentifier: "newThread", sender: self)
     }
     
     */
    // MARK: - Segues
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if segue.identifier == "showDetail" {
            if let indexPath = tableView.indexPathForSelectedRow {
                //let object = fetchedResultsController.object(at: indexPath)
                let controller = (segue.destination as! UINavigationController).topViewController as! DetailViewController
               // controller.detailItem = object
                controller.managedObjectContext = managedObjectContext
                controller.navigationItem.leftBarButtonItem = splitViewController?.displayModeButtonItem
                controller.navigationItem.leftItemsSupplementBackButton = true
                detailViewController = controller
            }
        }
            
        else if segue.identifier == "newThread" {
            let controller = segue.destination as! NewThreadView
            newThreadView = controller
            //threadView = newThreadView?.threadView
            
        }
    }
    
    // MARK: - Table View
    
    override func numberOfSections(in tableView: UITableView) -> Int {
       // return fetchedResultsController.sections?.count ?? 0
        return 0
    }

    override func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        //let sectionInfo = fetchedResultsController.sections![section]
        return threads!.count
    }

    override func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "Cell", for: indexPath)
        let thread = fetchedResultsController.object(at: indexPath)
        configureCell(cell, withThread: thread)
        return cell
    }

    override func tableView(_ tableView: UITableView, canEditRowAt indexPath: IndexPath) -> Bool {
        // Return false if you do not want the specified item to be editable.
        return true
    }

    override func tableView(_ tableView: UITableView, commit editingStyle: UITableViewCell.EditingStyle, forRowAt indexPath: IndexPath) {
        if editingStyle == .delete {
            let context = fetchedResultsController.managedObjectContext
            context.delete(fetchedResultsController.object(at: indexPath))
                
            do {
                try context.save()
            } catch {
                // Replace this implementation with code to handle the error appropriately.
                // fatalError() causes the application to generate a crash log and terminate. You should not use this function in a shipping application, although it may be useful during development.
                let nserror = error as NSError
                fatalError("Unresolved error \(nserror), \(nserror.userInfo)")
            }
        }
    }

    func configureCell(_ cell: UITableViewCell, withThread thread: Thread) {
        if thread.thread_name != nil {
            cell.textLabel!.text = thread.thread_name!
        }
        else {
            cell.textLabel!.text = thread.timestamp!.description
        }
    }


    // MARK: - Fetched results controller
    /*
    var _fetchedResultsController: NSFetchedResultsController<Thread>? = nil

    var fetchedResultsController: NSFetchedResultsController<Thread> {
        if _fetchedResultsController != nil {
            return _fetchedResultsController!
        }
        
        let fetchRequest: NSFetchRequest<Thread> = Thread.fetchRequest()
        
        // Set the batch size to a suitable number.
        fetchRequest.fetchBatchSize = 20
        
        // Edit the sort key as appropriate.
        let sortDescriptor = NSSortDescriptor(key: "timestamp", ascending: true)
        
        fetchRequest.sortDescriptors = [sortDescriptor]
        
        // Edit the section name key path and cache name if appropriate.
        // nil for section name key path means "no sections".
        let aFetchedResultsController = NSFetchedResultsController(fetchRequest: fetchRequest, managedObjectContext: self.managedObjectContext!, sectionNameKeyPath: nil, cacheName: "Master")
        aFetchedResultsController.delegate = self
        _fetchedResultsController = aFetchedResultsController
        
        do {
            try _fetchedResultsController!.performFetch()
        } catch {
             // Replace this implementation with code to handle the error appropriately.
             // fatalError() causes the application to generate a crash log and terminate. You should not use this function in a shipping application, although it may be useful during development. 
             let nserror = error as NSError
             fatalError("Unresolved error \(nserror), \(nserror.userInfo)")
        }
        
        return _fetchedResultsController!
    }
 
    //var _fetchedResultsController: NSFetchedResultsController<Thread>? = nil
    
    /*
    var _fetchedResultsController: NSFetchedResultsController<NSManagedObject>? = nil

    var fetchedResultsController: NSFetchedResultsController<NSManagedObject> {
        if _fetchedResultsController != nil {
            return _fetchedResultsController!
        }
        
        let fetchRequest: NSFetchRequest<NSManagedObject> = NSManagedObject.fetchRequest() as! NSFetchRequest<NSManagedObject>
        
        // Set the batch size to a suitable number.
        fetchRequest.fetchBatchSize = 20
        
        // Edit the sort key as appropriate.
        let sortDescriptor = NSSortDescriptor(key: "timestamp", ascending: true)
        
        fetchRequest.sortDescriptors = [sortDescriptor]
        
        // Edit the section name key path and cache name if appropriate.
        // nil for section name key path means "no sections".
        let aFetchedResultsController = NSFetchedResultsController(fetchRequest: fetchRequest, managedObjectContext: self.managedObjectContext!, sectionNameKeyPath: nil, cacheName: "Master")
        aFetchedResultsController.delegate = self
        _fetchedResultsController = aFetchedResultsController
        
        do {
            try _fetchedResultsController!.performFetch()
        } catch {
             // Replace this implementation with code to handle the error appropriately.
             // fatalError() causes the application to generate a crash log and terminate. You should not use this function in a shipping application, although it may be useful during development.
             let nserror = error as NSError
             fatalError("Unresolved error \(nserror), \(nserror.userInfo)")
        }
        
        return _fetchedResultsController!
    }
    */

    func controllerWillChangeContent(_ controller: NSFetchedResultsController<NSFetchRequestResult>) {
        tableView.beginUpdates()
    }

    func controller(_ controller: NSFetchedResultsController<NSFetchRequestResult>, didChange sectionInfo: NSFetchedResultsSectionInfo, atSectionIndex sectionIndex: Int, for type: NSFetchedResultsChangeType) {
        switch type {
            case .insert:
                tableView.insertSections(IndexSet(integer: sectionIndex), with: .fade)
            case .delete:
                tableView.deleteSections(IndexSet(integer: sectionIndex), with: .fade)
            default:
                return
        }
    }

    func controller(_ controller: NSFetchedResultsController<NSFetchRequestResult>, didChange anObject: Any, at indexPath: IndexPath?, for type: NSFetchedResultsChangeType, newIndexPath: IndexPath?) {
        switch type {
            case .insert:
                tableView.insertRows(at: [newIndexPath!], with: .fade)
            case .delete:
                tableView.deleteRows(at: [indexPath!], with: .fade)
            case .update:
                configureCell(tableView.cellForRow(at: indexPath!)!, withThread: anObject as! Thread)
            case .move:
                configureCell(tableView.cellForRow(at: indexPath!)!, withThread: anObject as! Thread)
                tableView.moveRow(at: indexPath!, to: newIndexPath!)
            default:
                return
        }
    }

    func controllerDidChangeContent(_ controller: NSFetchedResultsController<NSFetchRequestResult>) {
        tableView.endUpdates()
    }

    /*
     // Implementing the above methods to update the table view in response to individual changes may have performance implications if a large number of changes are made simultaneously. If this proves to be an issue, you can instead just implement controllerDidChangeContent: which notifies the delegate that all section and object changes have been processed.
     
     func controllerDidChangeContent(controller: NSFetchedResultsController) {
         // In the simplest, most efficient, case, reload the table view.
         tableView.reloadData()
     }
     */
 */
}


