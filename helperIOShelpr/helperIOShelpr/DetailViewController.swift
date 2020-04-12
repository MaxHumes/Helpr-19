//
//  DetailViewController.swift
//  helperIOShelpr
//
//  Created by Sam Horn on 4/4/20.
//  Copyright © 2020 Sam Horn. All rights reserved.
//

import UIKit
import CoreData

class DetailViewController: UITableViewController, NSFetchedResultsControllerDelegate {

    //@IBOutlet weak var detailDescriptionLabel: UILabel!
    @IBOutlet weak var postText: UITextField!
    

    @IBAction func onPost(_ sender: Any) {
        if postText.text != "" {
            
            let context = self.fetchedResultsController.managedObjectContext
            let newPost = Post(context: context)
            
            newPost.timestamp = Date()
            newPost.thread = detailItem
            newPost.message = postText.text
            
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
    
    var managedObjectContext: NSManagedObjectContext?
    
    

  // func configureView() {
        // Update the user interface for the detail item.
        //if let detail = detailItem {
          //  if let label = detailDescriptionLabel {
            //    label.text = detail.timestamp!.description
            //}
        //}
    //    if let split = splitViewController {
      //      let controllers = split.viewControllers
        //}
    //}

    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view.
        //configureView()
    }
    
    override func viewWillAppear(_ animated: Bool) {
        clearsSelectionOnViewWillAppear = splitViewController!.isCollapsed
        super.viewWillAppear(animated)
    }
    
    override func viewDidAppear(_ animated: Bool) {
        
        let context = self.fetchedResultsController.managedObjectContext
        do {
                try context.save()
            } catch {
                // Replace this implementation with code to handle the error appropriately.
                // fatalError() causes the application to generate a crash log and terminate. You should not use this function in a shipping application, although it may be useful during development.
                let nserror = error as NSError
                fatalError("Unresolved error \(nserror), \(nserror.userInfo)")
            }
        super.viewDidAppear(animated)
    }

    var detailItem: Thread? //{
          //didSet {
            // Update the view.
         //   configureView()
            //let context
          //}   // }
    
    // MARK: - Table View
    
    override func numberOfSections(in tableView: UITableView) -> Int {
           return fetchedResultsController.sections?.count ?? 0

    }
    
    override func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        let sectionInfo = fetchedResultsController.sections![section]
        return sectionInfo.numberOfObjects
    }
    
    override func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "Post Cell", for: indexPath)
        let post = fetchedResultsController.object(at: indexPath)
        //if post
        configureCell(cell, withPost: post)
        return cell
    }

    func configureCell(_ cell: UITableViewCell, withPost post: Post) {
        cell.textLabel!.textColor = UIColor(displayP3Red: 1.0, green: 0.0, blue: 0.0, alpha: 1.0)
        if post.author_name != nil {
            cell.textLabel!.text = String(format: "%s\n", post.author_name!)
        }
       // cell.textLabel!.text = String(format: "%s\n", post.author!.username!)
        cell.textLabel!.textColor = UIColor(displayP3Red: 0.0, green: 0.0, blue: 0.0, alpha: 0.0)
        if post.message != nil {
            cell.textLabel!.text = String(format: "%s", post.message!)
        }
        //cell.textLabel!.text = String(format: "%s", post.message!)
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
    // MARK: - Fetched Results Controller
    
    var _fetchedResultsController: NSFetchedResultsController<Post>? = nil

    var fetchedResultsController: NSFetchedResultsController<Post> {
        if _fetchedResultsController != nil {
            return _fetchedResultsController!
        }
        
        let fetchRequest: NSFetchRequest<Post> = Post.fetchRequest()
        
        fetchRequest.propertiesToFetch = ["thread"]
        
        
        // Set the batch size to a suitable number.
        fetchRequest.fetchBatchSize = 20
        
        // Edit the sort key as appropriate.
        let sortDescriptor = NSSortDescriptor(key: "timestamp", ascending: true)
        
        fetchRequest.sortDescriptors = [sortDescriptor]
        
        // Edit the section name key path and cache name if appropriate.
        // nil for section name key path means "no sections".
        let aFetchedResultsController = NSFetchedResultsController(fetchRequest: fetchRequest, managedObjectContext: self.managedObjectContext!, sectionNameKeyPath: nil, cacheName: "Thread")
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
                configureCell(tableView.cellForRow(at: indexPath!)!, withPost: anObject as! Post)
            case .move:
                configureCell(tableView.cellForRow(at: indexPath!)!, withPost: anObject as! Post)
                tableView.moveRow(at: indexPath!, to: newIndexPath!)
            default:
                return
        }
    }

    func controllerDidChangeContent(_ controller: NSFetchedResultsController<NSFetchRequestResult>) {
        tableView.endUpdates()
    }

    
    




}

